#include "pch.h"
#include "UnirDoc.xaml.h"
#if __has_include("UnirDoc.g.cpp")
#include "UnirDoc.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::PdfVysorWinUI::implementation
{
    UnirDoc::UnirDoc()
    {
        InitializeComponent();
    }

    
}
