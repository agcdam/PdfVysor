#include "pch.h"
#include "Ayuda.xaml.h"
#if __has_include("Ayuda.g.cpp")
#include "Ayuda.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::PdfVysorWinUI::implementation
{
    Ayuda::Ayuda()
    {
        InitializeComponent();
        this->NavigationCacheMode(NavigationCacheMode::Enabled);
    }

    void winrt::PdfVysorWinUI::implementation::Ayuda::Button_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::RoutedEventArgs const& e)
    {
        // instalacion de libreria
        system("echo \"Instalando libreria\" && powershell -command Install-Module PSWritePDF -Scope CurrentUser -Force && echo \"Libreria instalada\"");
    }
}



