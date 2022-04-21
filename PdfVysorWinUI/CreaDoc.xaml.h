#pragma once

#include "CreaDoc.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct CreaDoc : CreaDocT<CreaDoc>
    {
        CreaDoc();

       
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct CreaDoc : CreaDocT<CreaDoc, implementation::CreaDoc>
    {
    };
}
