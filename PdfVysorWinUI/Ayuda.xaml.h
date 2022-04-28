#pragma once

#include "Ayuda.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct Ayuda : AyudaT<Ayuda>
    {
        Ayuda();

        
        void Button_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::RoutedEventArgs const& e);
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct Ayuda : AyudaT<Ayuda, implementation::Ayuda>
    {
    };
}
