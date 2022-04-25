#include "pch.h"
#include "DivideDoc.xaml.h"
#if __has_include("DivideDoc.g.cpp")
#include "DivideDoc.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::PdfVysorWinUI::implementation
{
    DivideDoc::DivideDoc()
    {
        InitializeComponent();
        this->NavigationCacheMode(NavigationCacheMode::Enabled);
    }

    
}
