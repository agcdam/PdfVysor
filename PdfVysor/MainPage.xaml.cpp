//
// MainPage.xaml.cpp
// Implementación de la clase MainPage.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace PdfVysor;

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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

MainPage::MainPage() {
	InitializeComponent();
   buttonController->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
   scrollerPage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
   m_resolutionFactor = 1;
   m_zoomGlobal = 1;
   SetZomm();
}

//-----------------------------------------------------------------------------------------
/*
   Open new FileOpenPicker every time button loadDocument is clicked
*/
void PdfVysor::MainPage::OpenFile(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   loadDocument->IsEnabled = false;
   buttonController->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
   scrollerPage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
   
   m_document = nullptr;
   m_actualPage = 0; //index base 0

   auto picker = ref new FileOpenPicker();
   picker->FileTypeFilter->Append(".pdf");

   create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file) {
      if (file != nullptr) {
         return create_task(PdfDocument::LoadFromFileAsync(file, password->Password));
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
         
         buttonController->Visibility = Windows::UI::Xaml::Visibility::Visible;
		 scrollerPage->Visibility = Windows::UI::Xaml::Visibility::Visible;
         totalPagesBox->Text = m_document->PageCount.ToString();
         this->Update();
      }
   });

   loadDocument->IsEnabled = true;
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the first page
*/
void PdfVysor::MainPage::FirstPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   m_actualPage = 0;
   this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the previous page
*/
void PdfVysor::MainPage::PreviousPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   --m_actualPage;
   this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the next page
*/
void PdfVysor::MainPage::NextPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   ++m_actualPage;
   this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Navigate to the last page
*/
void PdfVysor::MainPage::LastPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   m_actualPage = m_document->PageCount - 1;
   this->Update();
}

//-----------------------------------------------------------------------------------------
/*
   Change the actual page to the page selected previously
*/
void PdfVysor::MainPage::Update() {
   //Shows the actual page index in base 1
   actualPageBox->Text = (m_actualPage + 1).ToString();
   if (m_actualPage == 0) {
      firstPage->IsEnabled = false;
      previosPage->IsEnabled = false;
   } else {
      firstPage->IsEnabled = true;
      previosPage->IsEnabled = true;
   }

   if (m_document != nullptr) {
      if (m_actualPage == m_document->PageCount - 1) {
         nextPage->IsEnabled = false;
         lastPage->IsEnabled = false;
      } else {
         nextPage->IsEnabled = true;
         lastPage->IsEnabled = true;
      }
   }

   this->ShowPage();
}

//-----------------------------------------------------------------------------------------
/*
   Render the actual page, from index (m_actualPage) to StreamMemory
*/
void PdfVysor::MainPage::ShowPage() {
   output->Source = nullptr;
   PdfPage^ page = m_document->GetPage(m_actualPage);
   auto stream = ref new InMemoryRandomAccessStream();

   IAsyncAction^ renderAction;

   auto options = ref new PdfPageRenderOptions();
   options->BackgroundColor = Windows::UI::Colors::White; //background color of page
   options->DestinationHeight = static_cast<unsigned int>(page->Size.Height * kZoomDefault);
   options->DestinationWidth = static_cast<unsigned int>(page->Size.Width * kZoomDefault);
   renderAction = page->RenderToStreamAsync(stream, options);

   create_task(renderAction).then([this, stream]() {
         auto src = ref new BitmapImage();
         output->Source = src;
         return create_task(src->SetSourceAsync(stream));
      });
}

//-----------------------------------------------------------------------------------------
/*
   Decrease the zoom value
*/
void PdfVysor::MainPage::ZoomOut(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   if (m_zoomGlobal > 0.25) {
      m_zoomGlobal -= 0.15;
   }
   zoom->Text = (m_zoomGlobal * 100).ToString();
   SetZomm();
}

//-----------------------------------------------------------------------------------------
/*
   Increase the zoom value
*/
void PdfVysor::MainPage::ZoomIn(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
   m_zoomGlobal += 0.15;

   //Shows the zoom level from base 1 to base 100
   zoom->Text = (m_zoomGlobal * 100).ToString();
   SetZomm();
}

//-----------------------------------------------------------------------------------------
/*
   Aply the zoom value to the zoom of the pages
*/
void PdfVysor::MainPage::SetZomm() {
   output->Height = (kBaseHeightImage * kZoomDefault * m_zoomGlobal);
   output->Width = (kBaseHeightImage * kZoomDefault * m_zoomGlobal);
}
