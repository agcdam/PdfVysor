﻿#include "pch.h"
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

    void DivideDoc::Button_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Microsoft::UI::Xaml::RoutedEventArgs const& e)
    {
        
        
        // ejemplo si la primera pagina es la 0 se podria dejar asi
        //system("powershell -command Split-PDF -FilePath C:\\Users\\agomez\\source\\repos\\PdfVysor\\test.pdf -OutputFolder C:\\Users\\agomez\\source\\repos\\PdfVysor\\PdfVysorWinUI\\Scripts\\ -SplitCount 5");

    }

    void DivideDoc::Log(std::string const& msg) {
        const char* charStr = msg.c_str();
        OutputDebugStringA(charStr);
        OutputDebugStringA("\n");
    }


}



