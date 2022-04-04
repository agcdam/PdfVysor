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

MainPage::MainPage()
{
	InitializeComponent();
   pagePanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}


void PdfVysor::MainPage::OpenFile(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   loadDocument->IsEnabled = false;
   pagePanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
   document = nullptr;
   actualPage = 0; //index base 0

   auto picker = ref new FileOpenPicker();
   picker->FileTypeFilter->Append(".pdf");

   create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file)
      {
      if (file != nullptr)
      {
         return create_task(PdfDocument::LoadFromFileAsync(file, password->Password));
      }
      else
      {
         return task_from_result(static_cast<PdfDocument^>(nullptr));
      }
   }).then([this](task<PdfDocument^> task)
   {
      try
      {
         document = task.get();
      }
      catch (Exception^ ex)
      {
         //Capturar excepcion
      }
      if (document != nullptr)
      {
         pagePanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
         totalPagesBox->Text = document->PageCount.ToString();
         this->Update();
      }
   });

   loadDocument->IsEnabled = true;
}


void PdfVysor::MainPage::FirstPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   actualPage = 0;
   this->Update();
}


void PdfVysor::MainPage::PreviousPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   --actualPage;
   this->Update();
}


void PdfVysor::MainPage::NextPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   ++actualPage;
   this->Update();
}


void PdfVysor::MainPage::LastPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
   actualPage = document->PageCount - 1;
   this->Update();
}

void PdfVysor::MainPage::Update() 
{
   actualPageBox->Text = (actualPage + 1).ToString();
   if (actualPage == 0)
   {
      firstPage->IsEnabled = false;
      previosPage->IsEnabled = false;
   }
   else
   {
      firstPage->IsEnabled = true;
      previosPage->IsEnabled = true;
   }

   if (document != nullptr)
   {
      if (actualPage == document->PageCount - 1)
      {
         nextPage->IsEnabled = false;
         lastPage->IsEnabled = false;
      }
      else
      {
         nextPage->IsEnabled = true;
         lastPage->IsEnabled = true;
      }  
   }

   this->ShowPage();
}

void PdfVysor::MainPage::ShowPage()
{
   output->Source = nullptr;
   PdfPage^ page = document->GetPage(actualPage);
   auto stream = ref new InMemoryRandomAccessStream();

   IAsyncAction^ renderAction;

   renderAction = page->RenderToStreamAsync(stream);

   create_task(renderAction).then([this, stream]()
      {
         auto src = ref new BitmapImage();
         output->Source = src;
         return create_task(src->SetSourceAsync(stream));
      });
}
