//
// MainPage.xaml.h
// Declaración de la clase MainPage.
//

#pragma once

#include "MainPage.g.h"

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
		double m_zoomGlobal;
      

		//Minimum page height used to calculate the zoom and size of output render
		const int kBaseHeightImage = 297;

		//Minimum page width used to calculate the zoom and size of output render
		const int kBaseWidthImage = 210;

		//Default zoom of a page used for the default zoom level (100 %)
		const int kZoomDefault = 4;
      

		void Update();
		void ShowPage();
		void SetZomm();
		void Log(Platform::String^ msg);
		bool IsNumber(const wchar_t *string);
		void SearchPage();

		void OpenFile(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void FirstPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void PreviousPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void NextPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void LastPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ZoomOut(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ZoomIn(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageLostFocus(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ActualPageKeyUp(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
	};
}
