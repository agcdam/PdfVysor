#pragma once

#include "MainPage.g.h"


namespace winrt::PdfVysor::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();


		//void FirstPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void FirstPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void PreviousPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);

		void ZoomOutVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void ZoomInVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void ActualPageLostFocusVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void ViewChangingVisor(IInspectable const& sender, winrt::Windows::UI::Xaml::Controls::ScrollViewerViewChangingEventArgs const& e);
		void RestoreZoomVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		fire_and_forget OpenFileVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void FirstPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void PreviousPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void NextPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void LastPageVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);

		void OpenFileDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void ActualPageLostFocusDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void NextPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void LastPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);

		/*void ActualPageKeyUpVisor(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::Input::KeyRoutedEventArgs const& e);
		void ActualPageKeyUpDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::Input::KeyRoutedEventArgs const& e);*/



	private:
		Windows::Data::Pdf::PdfDocument m_document {nullptr};
		Windows::Storage::StorageFile m_file {nullptr};

		int m_actualPage = 0;
		float m_zoomScroller = 1;
		std::vector<std::pair<Windows::UI::Xaml::Media::Imaging::BitmapImage, int>> m_pdfPages {};

		//Minimum page height used to calculate the zoom and size of output render
		const int kBaseHeightImage = 297;

		//Minimum page width used to calculate the zoom and size of output render
		const int kBaseWidthImage = 210;

		//Default zoom of a page used for the default zoom level (100 %)
		const int kZoomDefault = 4;

		//Rendering page quality
		const double kPageQualityRender = 2.5;



		void Update();
		fire_and_forget ShowPage();
		void Log(std::string const& msg);
		bool IsNumber(winrt::hstring const& str);
		void SetZoom();
		void SearchPage();
		bool ShowPageRendered();
		void LoadingPage(bool state);
		void ChangeVisibilityControls(Windows::UI::Xaml::Visibility state);

        
        
	};
}

namespace winrt::PdfVysor::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
