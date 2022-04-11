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
		/*std::vector<BitmapImage> m_pdfPages;*/
		std::vector<std::pair<BitmapImage^, int>> m_pdfPages;

		unsigned int m_indexRenderPages;

		//Minimum page height used to calculate the zoom and size of output render
		const int kBaseHeightImage = 297;

		//Minimum page width used to calculate the zoom and size of output render
		const int kBaseWidthImage = 210;

		//Default zoom of a page used for the default zoom level (100 %)
		const int kZoomDefault = 4;

		//Rendering page quality
		const double kPageQualityRender = 3.75;


		bool IsRendered(int page);

		void Update();
		void ShowPage();
		void Log(Platform::String^ msg);
		bool IsNumber(const wchar_t *string);
		void SetZoom();
		void SearchPage();
		bool ShowPageRendered();
		
		void RenderPages();
		

		void OpenFile(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void FirstPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void PreviousPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void NextPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void LastPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ZoomOut(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ZoomIn(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageLostFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageKeyUp(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
		void ViewChanging(Platform::Object^ sender, Windows::UI::Xaml::Controls::ScrollViewerViewChangingEventArgs^ e);
		void RestoreZoom(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
