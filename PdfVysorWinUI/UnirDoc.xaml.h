#pragma once

#include "UnirDoc.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct UnirDoc : UnirDocT<UnirDoc>
    {
        UnirDoc();

        
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct UnirDoc : UnirDocT<UnirDoc, implementation::UnirDoc>
    {
    };
}
