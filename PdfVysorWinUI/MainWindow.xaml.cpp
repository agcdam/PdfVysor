#include "pch.h"
#include "MainWindow.xaml.h"
#if __has_include("MainWindow.g.cpp")
#include "MainWindow.g.cpp"
#include <winrt/Microsoft.UI.Interop.h>
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;
using namespace Microsoft::UI::Xaml::Controls;

namespace winrt::PdfVysorWinUI::implementation
{
    MainWindow::MainWindow()
    {
        InitializeComponent();
        this->Title(kTitleWindow); //set the title of the app
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        When the navigation view it's loaded navigates to the page of vysor
    */
    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_Loaded(winrt::IInspectable const&, winrt::RoutedEventArgs const&)
    {
        nav().SelectedItem(nav().MenuItems().GetAt(0));
        ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Visor>(), *this);
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Change the page loaded on the frame depending on the item invoked
    */
    void winrt::PdfVysorWinUI::implementation::MainWindow::nav_ItemInvoked(winrt::NavigationView const&, winrt::NavigationViewItemInvokedEventArgs const& args)
    {
        if (args.InvokedItemContainer() != nullptr) {
            //obtain the tag of the new item
            IInspectable tag = args.InvokedItemContainer().Tag();
            hstring tagValue = unbox_value<hstring>(tag);
            if (tagValue == L"visor") {
                ContentFram().Navigate(xaml_typename<PdfVysorWinUI::Visor>(), *this);
            }
        }
    }
}
















