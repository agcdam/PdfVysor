#pragma once

#include "MainWindow.g.h"

namespace winrt::PdfVysorWinUI::implementation
{
    struct MainWindow : MainWindowT<MainWindow>
    {
        MainWindow();
        void nav_Loaded(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
        void nav_ItemInvoked(winrt::NavigationView const& sender, winrt::NavigationViewItemInvokedEventArgs const& args);

    private:
        const winrt::hstring kTitleWindow = L"PdfVysor";
    
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct MainWindow : MainWindowT<MainWindow, implementation::MainWindow>
    {
    };
}
