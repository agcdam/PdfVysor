#pragma once

#include "DivideDoc.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct DivideDoc : DivideDocT<DivideDoc>
    {
        DivideDoc();

        
        void Button_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::RoutedEventArgs const& e);
        void Log(std::string const& msg);
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct DivideDoc : DivideDocT<DivideDoc, implementation::DivideDoc>
    {
    };
}
