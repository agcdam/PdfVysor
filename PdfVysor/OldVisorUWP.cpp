#include "pch.h"
#include "OldVisorUWP.h"

//-----------------------------------------------------------------------------------------
	/*
	   Open new FileOpenPicker every time button loadDocumentVisor is clicked
	*/
	fire_and_forget winrt::PdfVysor::implementation::Visor::OpenFileVisor(IInspectable const&, RoutedEventArgs const&) {



		FileOpenPicker picker;


		picker.FileTypeFilter().Append(L".pdf");

		StorageFile file = co_await picker.PickSingleFileAsync();
		if (file != nullptr)
		{
			loadDocumentVisor().IsEnabled(false);
			PageControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
			scrollerPageVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
			ZoomControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Collapsed);
			//filePathVisor().Text(L"");

			m_document = nullptr;
			m_file = nullptr;
			m_actualPage = 0; //index base 0
			m_zoomScroller = 1;
			m_pdfPages.clear();

			m_file = file;
			try
			{
				m_document = co_await PdfDocument::LoadFromFileAsync(file, PasswordBox().Password());
			}
			catch (winrt::hresult_error const& ex)
			{
				//capturar excepcion
			}

			if (&m_document != nullptr)
			{
				totalPagesBoxVisor().Text(winrt::to_hstring(m_document.PageCount()));
				//filePathVisor().Text(m_file.Name());
				PageControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Visible);
				scrollerPageVisor().Visibility(Windows::UI::Xaml::Visibility::Visible);
				ZoomControllerVisor().Visibility(Windows::UI::Xaml::Visibility::Visible);

				this->Update();
			}
		}
		loadDocumentVisor().IsEnabled(true);
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the first page
	*/
	void winrt::PdfVysor::implementation::Visor::FirstPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		m_actualPage = 0;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the previous page
	*/
	void winrt::PdfVysor::implementation::Visor::PreviousPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		--m_actualPage;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the next page
	*/
	void winrt::PdfVysor::implementation::Visor::NextPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		++m_actualPage;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Navigate to the last page
	*/
	void winrt::PdfVysor::implementation::Visor::LastPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		m_actualPage = m_document.PageCount() - 1;
		this->Update();
	}

	//-----------------------------------------------------------------------------------------
	/*
	   Change the actual page to the page selected previously
	*/
	void winrt::PdfVysor::implementation::Visor::Update() {

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
	fire_and_forget winrt::PdfVysor::implementation::Visor::ShowPage() {
		//bloquear cambiar de pagina
		LoadingPage(true);
		outputVisor().Source(nullptr);
		// If the page is already rendered, shows the page that is rendered yet
		if (ShowPageRendered()) return;
		PdfPage page = m_document.GetPage(m_actualPage);
		InMemoryRandomAccessStream stream;
		auto options = PdfPageRenderOptions();
		options.BackgroundColor(Windows::UI::Colors::White()); //background color of page
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
	void winrt::PdfVysor::implementation::Visor::LoadingPage(bool enable) {
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
	bool winrt::PdfVysor::implementation::Visor::ShowPageRendered() {
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
	void winrt::PdfVysor::implementation::Visor::ZoomOutVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
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
	void winrt::PdfVysor::implementation::Visor::ZoomInVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
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
	void winrt::PdfVysor::implementation::Visor::Log(std::string const& msg) {
		const char* charStr = msg.c_str();
		OutputDebugStringA(charStr);
		OutputDebugStringA("\n");
	}

	//-----------------------------------------------------------------------------------------
	/*
		When actualPageBoxVisor loses pointer focus, it tries to find the page written on it
	*/
	void winrt::PdfVysor::implementation::Visor::ActualPageLostFocusVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
		SearchPage();
	}

	//-----------------------------------------------------------------------------------------
	/*
		When press enter or navigate with keys up/down, while focus is in actualPageBoxVisor search the page
	*/
	//void winrt::PdfVysor::implementation::Visor::ActualPageKeyUpVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::Input::KeyRoutedEventArgs const& e) {
	//	/*if (e.Key() == Windows::System::VirtualKey::Enter)
	//	{
	//		SearchPage();
	//	}*/
	//}

	//-----------------------------------------------------------------------------------------
	/*
		Search the page with the number written in actualPageBoxVisor
	*/
	void winrt::PdfVysor::implementation::Visor::SearchPage() {
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
	bool winrt::PdfVysor::implementation::Visor::IsNumber(winrt::hstring const& str) {
		std::string aux = winrt::to_string(str);
		for (char const& c : aux) {
			if (isdigit(c) == 0) return false;
		}
		return true;
	}

	//-----------------------------------------------------------------------------------------
	/*
		Updates the zoom text while aplying zoom with wheelmouse + ctrl
	*/
	void winrt::PdfVysor::implementation::Visor::ViewChangingVisor(IInspectable const& sender, ScrollViewerViewChangingEventArgs const& e)
	{
		m_zoomScroller = scrollerPageVisor().ZoomFactor();
		zoomVisor().Text(winrt::to_hstring((int)(m_zoomScroller * 100)) + L" %");
	}

	//-----------------------------------------------------------------------------------------
	/*
		Set zoom 100%
	*/
	void winrt::PdfVysor::implementation::Visor::RestoreZoomVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{
		m_zoomScroller = (float)1;
		SetZoom();
	}

	//-----------------------------------------------------------------------------------------
	/*
		Changes zoom and updates zoom text
	*/
	void winrt::PdfVysor::implementation::Visor::SetZoom() {
		zoomVisor().Text(winrt::to_hstring((int)(m_zoomScroller * 100)) + L" %");
		scrollerPageVisor().ChangeView(scrollerPageVisor().HorizontalOffset(), scrollerPageVisor().VerticalOffset(), m_zoomScroller);
	}