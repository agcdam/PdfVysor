#pragma once

#include "MainWindow.g.h"


namespace winrt::PdfVysorWinUI::implementation
{
    struct MainWindow : MainWindowT<MainWindow>
    {
        MainWindow();
        void nav_Loaded(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
        void nav_ItemInvoked(winrt::NavigationView const& sender, winrt::NavigationViewItemInvokedEventArgs const& args);
        void Button_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e);
        void nav_SelectionChanged(winrt::NavigationView const& sender, winrt::NavigationViewSelectionChangedEventArgs const& args);
    };
}

namespace winrt::PdfVysorWinUI::factory_implementation
{
    struct MainWindow : MainWindowT<MainWindow, implementation::MainWindow>
    {
    };
}
