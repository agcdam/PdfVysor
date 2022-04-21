#pragma once

#include "Ayuda.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct Ayuda : AyudaT<Ayuda>
    {
        Ayuda();

        
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct Ayuda : AyudaT<Ayuda, implementation::Ayuda>
    {
    };
}
