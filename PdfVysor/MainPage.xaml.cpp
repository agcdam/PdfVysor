//
// MainPage.xaml.cpp
// Implementación de la clase MainPage.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include <sstream>
#include <iostream>

#include <thread>
#include <chrono>

using namespace PdfVysor;

using std::cout;
using std::cin;
using std::endl;
using std::string;


using namespace Concurrency;
using namespace Windows::Storage;
using namespace Windows::Data::Pdf;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation::Numerics;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

MainPage::MainPage() {
	InitializeComponent();
	buttonControllerVisor->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	scrollerPageVisor->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	m_zoomScroller = scrollerPageVisor->ZoomFactor;
	SetZoom();
}



//-----------------------------------------------------------------------------------------
/*
   Open new FileOpenPicker every time button loadDocumentVisor is clicked
*/
void PdfVysor::MainPage::OpenFileVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	loadDocumentVisor->IsEnabled = false;
	buttonControllerVisor->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	scrollerPageVisor->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	filePathVisor->Text = "";

	m_document = nullptr;
	m_file = nullptr;
	m_pathFile = nullptr;
	m_actualPage = 0; //index base 0
	m_zoomScroller = 1;
	m_pdfPages.clear();

	auto picker = ref new FileOpenPicker();
	picker->FileTypeFilter->Append(".pdf");

	create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file) {
		
		if (file != nullptr) {
			m_file = file;
			return create_task(PdfDocument::LoadFromFileAsync(file, passwordVisor->Password));
		} else {
			return task_from_result(static_cast<PdfDocument^>(nullptr));
		}
	}).then([this](task<PdfDocument^> task) {
		try {
			m_document = task.get();
		} catch (Exception^ ex) {
			//Capturar excepcion
		}

		if (m_document != nullptr) {
			filePathVisor->Text = m_file->Name;
			buttonControllerVisor->Visibility = Windows::UI::Xaml::Visibility::Visible;
			scrollerPageVisor->Visibility = Windows::UI::Xaml::Visibility::Visible;
			totalPagesBoxVisor->Text = m_document->PageCount.ToString();
			this->Update();
		}
	});
	loadDocumentVisor->IsEnabled = true;
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the first page
*/
void PdfVysor::MainPage::FirstPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	m_actualPage = 0;
	this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the previous page
*/
void PdfVysor::MainPage::PreviousPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	--m_actualPage;
	this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the next page
*/
void PdfVysor::MainPage::NextPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	++m_actualPage;
	this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the last page
*/
void PdfVysor::MainPage::LastPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	m_actualPage = m_document->PageCount - 1;
	this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Change the actual page to the page selected previously
*/
void PdfVysor::MainPage::Update() {
	
	if (m_actualPage == 0) {
		firstPageVisor->IsEnabled = false;
		previousPageVisor->IsEnabled = false;
	} else {
		firstPageVisor->IsEnabled = true;
		previousPageVisor->IsEnabled = true;
	}

	if (m_document != nullptr) {
		if (m_actualPage == m_document->PageCount - 1) {
			nextPageVisor->IsEnabled = false;
			lastPageVisor->IsEnabled = false;
		} else {
			nextPageVisor->IsEnabled = true;
			lastPageVisor->IsEnabled = true;
		}
	}

	this->ShowPage();
}

//-----------------------------------------------------------------------------------------
/*
   Render the actual page, from index (m_actualPage) to StreamMemory
*/
void PdfVysor::MainPage::ShowPage() {
	//bloquear cambiar de pagina
	LoadingPage(true);
	outputVisor->Source = nullptr;
	// If the page is already rendered, don't do nothing
	if (ShowPageRendered()) return;
	PdfPage^ page = m_document->GetPage(m_actualPage);
	auto stream = ref new InMemoryRandomAccessStream();
	IAsyncAction^ renderAction;
	auto options = ref new PdfPageRenderOptions();
	options->BackgroundColor = Windows::UI::Colors::White; //background color of page
	options->DestinationHeight = static_cast<unsigned int>(page->Size.Height * kPageQualityRender);
	options->DestinationWidth = static_cast<unsigned int>(page->Size.Width * kPageQualityRender);
	renderAction = page->RenderToStreamAsync(stream, options);
	auto src = ref new BitmapImage();
	outputVisor->Source = src;
	outputVisor->Height = kBaseHeightImage * kZoomDefault;
	outputVisor->Width = kBaseWidthImage * kZoomDefault;
	//Shows the actual page index in base 1
	actualPageBoxVisor->Text = (m_actualPage + 1).ToString();
	//m_pdfPages[m_actualPage] = src;
	m_pdfPages.push_back({ src, m_actualPage });
	//desbloquear cambiar de pagina
	

	create_task(renderAction).then([=]() {
		LoadingPage(false);
		return create_task(src->SetSourceAsync(stream));
		});
	
}

//-----------------------------------------------------------------------------------------
/*
	Disables controls on screen while page is rendering
*/
void PdfVysor::MainPage::LoadingPage(bool enable) {
	enable = !enable;
	loadDocumentVisor->IsEnabled = enable;
	if (m_actualPage > 0) {
		firstPageVisor->IsEnabled = enable;
		previousPageVisor->IsEnabled = enable;
	} else {
		firstPageVisor->IsEnabled = false;
		previousPageVisor->IsEnabled = false;
	}
	
	actualPageBoxVisor->IsEnabled = enable;
	if (m_actualPage < m_document->PageCount - 1)
	{
		nextPageVisor->IsEnabled = enable;
		lastPageVisor->IsEnabled = enable;
	}
	else
	{
		nextPageVisor->IsEnabled = false;
		lastPageVisor->IsEnabled = false;
	}
	
	zoomOutVisor->IsEnabled = enable;
	zoomInVisor->IsEnabled = enable;
	restoreZoomVisor->IsEnabled = enable;
	progressBarVisor->IsActive = !enable;
	if (enable) {
		progressBarVisor->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	} else {
		progressBarVisor->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

//-----------------------------------------------------------------------------------------
/*
	Looks if the actual page is already rendered
	If it's true set the resource in the outputVisor
*/
bool PdfVysor::MainPage::ShowPageRendered() {
	if (!m_pdfPages.empty())
	{
		for (const auto& x : m_pdfPages) {
			if (x.second == m_actualPage)
			{
				//Shows the actual page index in base 1
				actualPageBoxVisor->Text = (m_actualPage + 1).ToString();
				outputVisor->Source = x.first;
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
void PdfVysor::MainPage::ZoomOutVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	if ((m_zoomScroller - scrollerPageVisor->MinZoomFactor) > 0.15)
	{
		m_zoomScroller -= 0.15f;
	}
	else if (m_zoomScroller > scrollerPageVisor->MinZoomFactor)
	{
		m_zoomScroller -= scrollerPageVisor->MinZoomFactor;
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
void PdfVysor::MainPage::ZoomInVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	if ((scrollerPageVisor->MaxZoomFactor - m_zoomScroller) > 0.15)
	{
		m_zoomScroller += 0.15f;
	}
	else if (scrollerPageVisor->MaxZoomFactor > m_zoomScroller)
	{
		m_zoomScroller += (scrollerPageVisor->MaxZoomFactor - m_zoomScroller);
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
void PdfVysor::MainPage::Log(Platform::String^ msg) {
	std::wstring fooW(msg->Begin());
	std::string fooA(fooW.begin(), fooW.end());
	const char* charStr = fooA.c_str();
	OutputDebugStringA(charStr);
	OutputDebugStringA("\n");
}

//-----------------------------------------------------------------------------------------
/*
	When actualPageBoxVisor loses pointer focus, it tries to find the page written on it
*/
void PdfVysor::MainPage::ActualPageLostFocusVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	SearchPage();
}

//-----------------------------------------------------------------------------------------
/*
	When press enter or navigate with keys up/down, while focus is in actualPageBoxVisor search the page
*/
void PdfVysor::MainPage::ActualPageKeyUpVisor(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e) {
	if (e->Key == Windows::System::VirtualKey::Enter)
	{
		SearchPage();
	}
}

//-----------------------------------------------------------------------------------------
/*
	Search the page with the number written in actualPageBoxVisor
*/
void PdfVysor::MainPage::SearchPage() {
	try {
		const wchar_t *string = actualPageBoxVisor->Text->Data();
		unsigned int newPage;
		IsNumber(string) ? newPage = std::stoi(string) : throw std::exception();
		--newPage; // set index base from 1 to 0
		// if the new page isn't on the range of pages throw new exception
		if (newPage < 0 || newPage >= m_document->PageCount) throw std::exception();
		// if the new page it's the same than actual return and does nothing
		if (m_actualPage == newPage) {
			actualPageBoxVisor->Text = (m_actualPage + 1).ToString();
			return;
		}
		m_actualPage = newPage;
		this->Update();
	}
	catch (const std::exception&) {
		actualPageBoxVisor->Text = (m_actualPage + 1).ToString();
		animationErrorVisor->Begin();
	}
}

//-----------------------------------------------------------------------------------------
/*
	Checks if the actualPageBoxVisor is a number
*/
bool PdfVysor::MainPage::IsNumber(const wchar_t *str) {
	std::wstring ws(str);
	std::string aux(ws.begin(), ws.end());
	for (char const &c : aux) {
		if (isdigit(c) == 0) return false;
	}
	return true;
}

//-----------------------------------------------------------------------------------------
/*
	Updates the zoom text while aplying zoom with wheelmouse + ctrl
*/
void PdfVysor::MainPage::ViewChangingVisor(Platform::Object^ sender, Windows::UI::Xaml::Controls::ScrollViewerViewChangingEventArgs^ e)
{
	m_zoomScroller = scrollerPageVisor->ZoomFactor;
	zoomVisor->Text = ((int)(m_zoomScroller * 100)).ToString() + " %";
}

//-----------------------------------------------------------------------------------------
/*
	Set zoom 100%
*/
void PdfVysor::MainPage::RestoreZoomVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_zoomScroller = (float) 1;
	SetZoom();
}

//-----------------------------------------------------------------------------------------
/*
	Changes zoom and updates zoom text
*/
void PdfVysor::MainPage::SetZoom() {
	zoomVisor->Text = ((int)(m_zoomScroller * 100)).ToString() + " %";
	scrollerPageVisor->ChangeView(scrollerPageVisor->HorizontalOffset, scrollerPageVisor->VerticalOffset, m_zoomScroller);
}


void PdfVysor::MainPage::OpenFileDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//open file for file divider
}

void PdfVysor::MainPage::ActualPageKeyUpDiv(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e)
{
	
}

void PdfVysor::MainPage::ActualPageLostFocusDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) 
{

}

void PdfVysor::MainPage::FirstPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

}

void PdfVysor::MainPage::PreviousPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	
}

void PdfVysor::MainPage::NextPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	
}

void PdfVysor::MainPage::LastPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
	
}
