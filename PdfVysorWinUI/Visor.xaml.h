#pragma once

#include "Visor.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    enum class Page { First, Previous, Next, Last, Search };
    struct Visor : VisorT<Visor>
    {
        Visor();

    private:
        winrt::PdfDocument m_pdfDocument{ nullptr };
        unsigned int m_actualPage = 0;
        float m_zoomScroller = 1;
        std::vector<std::pair<BitmapImage, int>> m_pagesCache{};
        bool m_documentIsLoaded{ false };

        const int kHeightImage = 1188; // base height of the pdf page
        const int kWidthImage = 840; // base width of the pdf page
        const double kPageQualityRender = 2.5; // quality of the page's render

        // zoom level based on 1 as 100% (in the indicators is on % base)
        const float kMinZoom = 0.20f; // min factor of zoom level in the scroller page
        const float kMaxZoom = 5.0f; // max factor of zoom level in the scroller page
        const float kDefaultZoomValue = 1.0f; // default zoom value 
        const float kStepFrequencyZoom = 0.10f; // every click of the buttons increase o decrease 0.10 (10%) zoom value


        void PageControllersVisibility(winrt::Visibility const& visibility);
        void NewPdfLoaded(PdfDocument const& newDoc);
        bool IsNumber(winrt::hstring const& str);
        bool IsInRange(winrt::hstring const& str);
        void CheckPageControls();
        void UpdatePageValue(Page page);
        fire_and_forget RenderPage();
        BitmapImage GetPageRendered(unsigned int const& page);
        void ShowPage(const BitmapImage& src);
        void UpdateScrollViewer();
        void UpdateZoomValue();
        void AdjustPageZoom();
        void UncheckAdjustZoom();

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
        void ScrollerPage_ViewChanging(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::Controls::ScrollViewerViewChangingEventArgs const& e);
        void ScrollerPage_SizeChanged(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::SizeChangedEventArgs const& e);
        void AdjustZoom_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::RoutedEventArgs const& e);
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct Visor : VisorT<Visor, implementation::Visor>
    {
    };
}
