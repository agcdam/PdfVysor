#pragma once

#include "DivideDoc.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct DivideDoc : DivideDocT<DivideDoc>
    {
        DivideDoc();

        
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct DivideDoc : DivideDocT<DivideDoc, implementation::DivideDoc>
    {
    };
}
