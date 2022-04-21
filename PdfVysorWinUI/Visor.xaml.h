#pragma once

#include "Visor.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct Visor : VisorT<Visor>
    {
        Visor();

        
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct Visor : VisorT<Visor, implementation::Visor>
    {
    };
}
