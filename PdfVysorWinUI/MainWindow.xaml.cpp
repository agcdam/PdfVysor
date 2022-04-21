#include "pch.h"
#include "MainWindow.xaml.h"
#if __has_include("MainWindow.g.cpp")
#include "MainWindow.g.cpp"
#include <winrt/Microsoft.UI.Interop.h>
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;
using namespace Microsoft::UI::Xaml::Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::PdfVysorWinUI::implementation
{
    MainWindow::MainWindow()
    {
        InitializeComponent();
    }

    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_Loaded(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {

    }


    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_ItemInvoked(winrt::NavigationView const& sender, winrt::NavigationViewItemInvokedEventArgs const& args)
    {

    }

    void winrt::PdfVysorWinUI::implementation::MainWindow::Button_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        
        ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Ayuda>(), *this);
    }
}













