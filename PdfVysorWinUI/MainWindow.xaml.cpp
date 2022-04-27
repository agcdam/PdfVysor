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
        this->Title(kTitleWindow);
    }

    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_Loaded(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        nav().SelectedItem(nav().MenuItems().GetAt(0));
        ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Visor>(), *this);
    }


    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_ItemInvoked(winrt::NavigationView const& sender, winrt::NavigationViewItemInvokedEventArgs const& args)
    {
        if (args.InvokedItemContainer() != nullptr) {
            IInspectable tag = args.InvokedItemContainer().Tag();
            hstring tagValue = unbox_value<hstring>(tag);
            if (tagValue == L"visor") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Visor>(), *this);
            } else if (tagValue == L"divide") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::DivideDoc>(), *this);
            } else if (tagValue == L"unir") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::UnirDoc>(), *this);
            } else if (tagValue == L"newDocumentacion") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::CreaDoc>(), *this);
            } else if (tagValue == L"ayuda") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Ayuda>(), *this);
            }
            
            
        }
    }

    void winrt::PdfVysorWinUI::implementation::MainWindow::InfoBarButton_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        nav().SelectedItem(nav().MenuItems().GetAt(3));
        ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Ayuda>(), *this);
        InfoBar().IsOpen(false);
    }
}
















