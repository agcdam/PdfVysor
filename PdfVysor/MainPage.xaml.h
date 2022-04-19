//
// MainPage.xaml.h
// Declaración de la clase MainPage.
//

#pragma once

#include "MainPage.g.h"

using namespace Windows::UI::Xaml::Media::Imaging;

namespace PdfVysor {
	/// <summary>
	/// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
	/// </summary>
	public ref class MainPage sealed {
	public:
		MainPage();

	private:
		Windows::Data::Pdf::PdfDocument^ m_document;
		Windows::Storage::StorageFile^ m_file;
		Platform::String^ m_pathFile;
		
		int m_actualPage;
		float m_zoomScroller;
		std::vector<std::pair<BitmapImage^, int>> m_pdfPages;

		//Minimum page height used to calculate the zoom and size of output render
		const int kBaseHeightImage = 297;

		//Minimum page width used to calculate the zoom and size of output render
		const int kBaseWidthImage = 210;

		//Default zoom of a page used for the default zoom level (100 %)
		const int kZoomDefault = 4;

		//Rendering page quality
		const double kPageQualityRender = 2.5;


		
		void Update();
		void ShowPage();
		void Log(Platform::String^ msg);
		bool IsNumber(const wchar_t *string);
		void SetZoom();
		void SearchPage();
		bool ShowPageRendered();
		void LoadingPage(bool state);
		

		void ZoomOutVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ZoomInVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageLostFocusVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageKeyUpVisor(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
		void ViewChangingVisor(Platform::Object^ sender, Windows::UI::Xaml::Controls::ScrollViewerViewChangingEventArgs^ e);
		void RestoreZoomVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void OpenFileVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void FirstPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void PreviousPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void NextPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void LastPageVisor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		void OpenFileDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageKeyUpDiv(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
		void ActualPageLostFocusDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void FirstPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void PreviousPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void NextPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void LastPageDiv(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
