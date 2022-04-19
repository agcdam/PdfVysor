#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"

using namespace winrt;
using namespace winrt::Windows::Data::Pdf;
using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::Storage;
using namespace winrt::Windows::Storage::Streams;
using namespace winrt::Windows::Storage::Pickers;
using namespace winrt::Windows::UI::Core;
using namespace winrt::Windows::UI::Xaml;
using namespace winrt::Windows::UI::Xaml::Media::Imaging;
using namespace winrt::Windows::UI::Xaml::Input;
using namespace winrt::Windows::UI::Xaml::Controls;

namespace winrt::PdfVysor::implementation
{
	MainPage::MainPage() {
		InitializeComponent();
		buttonControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
		scrollerPageVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
		filePathVisor().Text(L"");
	}


	//-----------------------------------------------------------------------------------------
	/*
	   Open new FileOpenPicker every time button loadDocumentVisor is clicked
	*/
	fire_and_forget winrt::PdfVysor::implementation::MainPage::OpenFileVisor(IInspectable const&, RoutedEventArgs const&) {

		loadDocumentVisor().IsEnabled(false);
		buttonControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
		scrollerPageVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
		filePathVisor().Text(L"");

		m_document = nullptr;
		m_file = nullptr;
		m_actualPage = 0; //index base 0
		m_zoomScroller = 1;
		m_pdfPages.clear();

		FileOpenPicker picker;
		
		
		picker.FileTypeFilter().Append(L".pdf");

		StorageFile file = co_await picker.PickSingleFileAsync();
		if (file != nullptr)
		{
			m_file = file;
			try
			{
				m_document = co_await PdfDocument::LoadFromFileAsync(file, PasswordBox().Password());
			}
			catch (winrt::hresult_error const& ex)
			{
				
			}

			if (&m_document != nullptr)
			{
				totalPagesBoxVisor().Text(winrt::to_hstring(m_document.PageCount()));
				filePathVisor().Text(m_file.Name());
				buttonControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Visible());
				scrollerPageVisor().Visibility(Windows::UI::Xaml::Visibility::Visible());
				totalPagesBoxVisor().Text(winrt::to_hstring(m_document.PageCount()));
				this->Update();
			}
		}
		loadDocumentVisor().IsEnabled(true);

		//create_task(picker->PickSingleFileAsync()).then([this](StorageFile* file) {

		//	if (file != nullptr) {
		//		m_file = file;
		//		return create_task(PdfDocument::LoadFromFileAsync(file, passwordVisor->Password));
		//	}
		//	else {
		//		return task_from_result(static_cast<PdfDocument^>(nullptr));
		//	}
		//	}).then([this](task<PdfDocument*> task) {
		//		try {
		//			m_document = task.get();
		//		}
		//		catch (Exception^ ex) {
		//			//Capturar excepcion
		//		}

		//		if (m_document != nullptr) {
		//			filePathVisor->Text = m_file->Name;
		//			buttonControllerVisor->Visibility = Windows::UI::Xaml::Visibility::Visible;
		//			scrollerPageVisor()Visibility = Windows::UI::Xaml::Visibility::Visible;
		//			totalPagesBoxVisor->Text = m_document.PageCount.ToString();
		//			this->Update();
		//		}
		//		});
		//	loadDocumentVisor().IsEnabled(true);
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the first page
	*/
	void winrt::PdfVysor::implementation::MainPage::FirstPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		m_actualPage = 0;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the previous page
	*/
	void winrt::PdfVysor::implementation::MainPage::PreviousPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		--m_actualPage;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the next page
	*/
	void winrt::PdfVysor::implementation::MainPage::NextPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		++m_actualPage;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the last page
	*/
	void winrt::PdfVysor::implementation::MainPage::LastPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		m_actualPage = m_document.PageCount() - 1;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Change the actual page to the page selected previously
	*/
	void winrt::PdfVysor::implementation::MainPage::Update() {

		if (m_actualPage == 0) {
			firstPageVisor().IsEnabled(false);
			previousPageVisor().IsEnabled(false);
		}
		else {
			firstPageVisor().IsEnabled(true);
			previousPageVisor().IsEnabled(true);
		}

		if (m_document != nullptr) {
			if (m_actualPage == m_document.PageCount() - 1) {
				nextPageVisor().IsEnabled(false);
				lastPageVisor().IsEnabled(false);
			}
			else {
				nextPageVisor().IsEnabled(true);
				lastPageVisor().IsEnabled(true);
			}
		}

		this->ShowPage();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Render the actual page, from index (m_actualPage) to StreamMemory
	*/
	fire_and_forget winrt::PdfVysor::implementation::MainPage::ShowPage() {
		//bloquear cambiar de pagina
		LoadingPage(true);
		outputVisor().Source(nullptr);
		// If the page is already rendered, don't do nothing
		if (ShowPageRendered()) return;
		PdfPage page = m_document.GetPage(m_actualPage);
		InMemoryRandomAccessStream stream;
		IAsyncAction* renderAction;
		auto options = PdfPageRenderOptions();
		options.BackgroundColor(Windows::UI::Colors::White()) ; //background color of page
		options.DestinationHeight(static_cast<unsigned int>(page.Size().Height * kPageQualityRender));
		options.DestinationHeight(static_cast<unsigned int>(page.Size().Width * kPageQualityRender));
		co_await page.RenderToStreamAsync(stream, options);
		BitmapImage src;
		outputVisor().Source(src);
		outputVisor().Height(kBaseHeightImage * kZoomDefault);
		outputVisor().Width(kBaseWidthImage * kZoomDefault);
		//Shows the actual page index in base 1
		actualPageBoxVisor().Text(winrt::to_hstring(m_actualPage + 1));
		//m_pdfPages[m_actualPage] = src;
		m_pdfPages.push_back({ src, m_actualPage });
		//desbloquear cambiar de pagina
		co_await src.SetSourceAsync(stream);
		LoadingPage(false);

	}


	//-----------------------------------------------------------------------------------------
	/*
		Disables controls on screen while page is rendering
	*/
	void winrt::PdfVysor::implementation::MainPage::LoadingPage(bool enable) {
		enable = !enable;
		loadDocumentVisor().IsEnabled(enable);
		if (m_actualPage > 0) {
			firstPageVisor().IsEnabled(enable);
			previousPageVisor().IsEnabled(enable);
		}
		else {
			firstPageVisor().IsEnabled(false);
			previousPageVisor().IsEnabled(false);
		}

		actualPageBoxVisor().IsEnabled(enable);
		if (m_actualPage < m_document.PageCount() - 1)
		{
			nextPageVisor().IsEnabled(enable);
			lastPageVisor().IsEnabled(enable);
		}
		else
		{
			nextPageVisor().IsEnabled(false);
			lastPageVisor().IsEnabled(false);
		}

		zoomOutVisor().IsEnabled(enable);
		zoomInVisor().IsEnabled(enable);
		restoreZoomVisor().IsEnabled(enable);
		progressBarVisor().IsActive(!enable);
		if (enable) {
			progressBarVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
		}
		else {
			progressBarVisor().Visibility(Windows::UI::Xaml::Visibility::Visible);
		}
	}

	//-----------------------------------------------------------------------------------------
	/*
		Looks if the actual page is already rendered
		If it's true set the resource in the outputVisor
	*/
	bool winrt::PdfVysor::implementation::MainPage::ShowPageRendered() {
		if (!m_pdfPages.empty())
		{
			for (const auto& x : m_pdfPages) {
				if (x.second == m_actualPage)
				{
					//Shows the actual page index in base 1
					actualPageBoxVisor().Text(winrt::to_hstring(m_actualPage + 1));
					outputVisor().Source(x.first);
					LoadingPage(false);
					return true;
				}
			}
		}
		return false;
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Decrease the zoom value
	*/
	void winrt::PdfVysor::implementation::MainPage::ZoomOutVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		if ((m_zoomScroller - scrollerPageVisor().MinZoomFactor()) > 0.15)
		{
			m_zoomScroller -= 0.15f;
		}
		else if (m_zoomScroller > scrollerPageVisor().MinZoomFactor())
		{
			m_zoomScroller -= scrollerPageVisor().MinZoomFactor();
		}
		else
		{
			return;
		}
		SetZoom();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Increase the zoom value
	*/
	void winrt::PdfVysor::implementation::MainPage::ZoomInVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		if ((scrollerPageVisor().MaxZoomFactor() - m_zoomScroller) > 0.15)
		{
			m_zoomScroller += 0.15f;
		}
		else if (scrollerPageVisor().MaxZoomFactor() > m_zoomScroller)
		{
			m_zoomScroller += (scrollerPageVisor().MaxZoomFactor() - m_zoomScroller);
		}
		else
		{
			return;
		}
		SetZoom();
	}

	//-----------------------------------------------------------------------------------------
	/*
		Shows Logs into debug console
	*/
	void winrt::PdfVysor::implementation::MainPage::Log(std::string const& msg) {
		const char* charStr = msg.c_str();
		OutputDebugStringA(charStr);
		OutputDebugStringA("\n");
	}

	//-----------------------------------------------------------------------------------------
	/*
		When actualPageBoxVisor loses pointer focus, it tries to find the page written on it
	*/
	void winrt::PdfVysor::implementation::MainPage::ActualPageLostFocusVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		SearchPage();
	}

	//-----------------------------------------------------------------------------------------
	/*
		When press enter or navigate with keys up/down, while focus is in actualPageBoxVisor search the page
	*/
	//void winrt::PdfVysor::implementation::MainPage::ActualPageKeyUpVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::Input::KeyRoutedEventArgs const& e) {
	//	/*if (e.Key() == Windows::System::VirtualKey::Enter)
	//	{
	//		SearchPage();
	//	}*/
	//}

	//-----------------------------------------------------------------------------------------
	/*
		Search the page with the number written in actualPageBoxVisor
	*/
	void winrt::PdfVysor::implementation::MainPage::SearchPage() {
		try {
			const winrt::hstring string = actualPageBoxVisor().Text();
			int newPage;
			IsNumber(string) ? newPage = std::stoi(winrt::to_string(string)) : throw std::exception();
			--newPage; // set index base from 1 to 0
			// if the new page isn't on the range of pages throw new exception
			if (newPage < 0 || newPage >= m_document.PageCount()) throw std::exception();
			// if the new page it's the same than actual return and does nothing
			if (m_actualPage == newPage) {
				actualPageBoxVisor().Text(winrt::to_hstring((m_actualPage + 1)));
				return;
			}
			m_actualPage = newPage;
			this->Update();
		}
		catch (const std::exception&) {
			actualPageBoxVisor().Text(winrt::to_hstring(m_actualPage + 1));
			//animationErrorVisor().Begin();
		}
	}

	//-----------------------------------------------------------------------------------------
	/*
		Checks if the actualPageBoxVisor is a number
	*/
	bool winrt::PdfVysor::implementation::MainPage::IsNumber(winrt::hstring const& str) {
		for (char const& c : str) {
			if (isdigit(c) == 0) return false;
		}
		return true;
	}

	//-----------------------------------------------------------------------------------------
	/*
		Updates the zoom text while aplying zoom with wheelmouse + ctrl
	*/
	void winrt::PdfVysor::implementation::MainPage::ViewChangingVisor(IInspectable const& sender, ScrollViewerViewChangingEventArgs const& e)
	{
		m_zoomScroller = scrollerPageVisor().ZoomFactor();
		zoomVisor().Text(winrt::to_hstring((int)(m_zoomScroller * 100)) + L" %");
	}

	//-----------------------------------------------------------------------------------------
	/*
		Set zoom 100%
	*/
	void winrt::PdfVysor::implementation::MainPage::RestoreZoomVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{
		m_zoomScroller = (float)1;
		SetZoom();
	}

	//-----------------------------------------------------------------------------------------
	/*
		Changes zoom and updates zoom text
	*/
	void winrt::PdfVysor::implementation::MainPage::SetZoom() {
		zoomVisor().Text(winrt::to_hstring((int)(m_zoomScroller * 100)) + L" %");
		scrollerPageVisor().ChangeView(scrollerPageVisor().HorizontalOffset(), scrollerPageVisor().VerticalOffset(), m_zoomScroller);
	}


	void winrt::PdfVysor::implementation::MainPage::OpenFileDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{
		//open file for file divider
	}

	void MainPage::ActualPageLostFocusDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{

	}

	void winrt::PdfVysor::implementation::MainPage::FirstPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{

	}

	void winrt::PdfVysor::implementation::MainPage::PreviousPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {

	}

	void winrt::PdfVysor::implementation::MainPage::NextPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {

	}

	void winrt::PdfVysor::implementation::MainPage::LastPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {

	}

}
