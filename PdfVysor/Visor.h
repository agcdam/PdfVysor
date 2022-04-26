#pragma once

#include "Visor.g.h"

namespace winrt::PdfVysor::implementation
{
    struct Visor : VisorT<Visor>
    {
        Visor();

	private:
		winrt::PdfDocument m_pdfDocument{ nullptr };
		unsigned int m_actualPage = 0;
		int m_zoomScroller = 100;
		std::vector<std::pair<BitmapImage, int>> m_pagesCache{};
		const int kHeightImage = 1188;
		const int kWidthImage = 840;
		const double kPageQualityRender = 2.5;
		const int kminZoom = 20;
		const int kmaxZoom = 500;

		void Log(std::string const& msg);
		void PageControllersVisibility(winrt::Visibility const& visibility);
		void NewPdfLoaded(PdfDocument const& newDoc);
		bool IsNumber(winrt::hstring const& str);
		bool IsInRange(winrt::hstring const& str);
		void CheckPageControls();
		fire_and_forget RenderPage();
		BitmapImage GetPageRendered(unsigned int const& page);
		void ShowPage(const BitmapImage& src);

	public:
		fire_and_forget OpenFileButton_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void Slider_ValueChanged(winrt::IInspectable const& sender, winrt::Primitives::RangeBaseValueChangedEventArgs const& e);
		void FirstPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void PreviousPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void SearchPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void NextPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void LastPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void ZoomOut_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void ZoomIn_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void ResetZoom_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
		void FlyoutZoom_Opening(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::Foundation::IInspectable const& e);

		//void FirstPageDiv(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
	};
}

namespace winrt::PdfVysor::factory_implementation
{
    struct Visor : VisorT<Visor, implementation::Visor>
    {
    };
}
