#include "pch.h"
#include "DivideDoc.xaml.h"
#if __has_include("DivideDoc.g.cpp")
#include "DivideDoc.g.cpp"
#endif

#include <iostream>
#include <string>
#include <filesystem>


using std::cout; using std::cin;
using std::endl; using std::string;


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
        //C:\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\DividirPdf.ps1 -numeroPaginas 5 -outName hola

        /*ShellExecuteA(NULL, 
            "runas", 
            "C:\\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\DividirPdf.ps1", 
            "C:\\Users\agomez\Downloads\Test.pdf -numeroPaginas 5 -outName hola", 
            "C:\\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\\",
            SW_SHOWNORMAL);*/

        //system("powershell -command C:\\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\DividirPdf.ps1 C:\\Users\agomez\Downloads\Test.pdf -numeroPaginas 5 -outName hola");
        //system("powershell.exe -command \"& { C:\\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\DividirPdf.ps1 C:\\Users\agomez\Downloads\Test.pdf -numeroPaginas 5 -outName hola }\"");
        //system("powershell -command sleep 2"); //funciona
        //system("powershell -command C:\\Users\agomez\source\repos\PdfVysor\PdfVysorWinUI\Scripts\DividirPdf.ps1 C:\\Users\agomez\Downloads\Test.pdf -numeroPaginas 5 -outName hola");
        system("powershell -command .\\PdfVysorWinUI\\Scripts\\DividirPdf.ps1 .\\test.pdf -numeroPaginas 5 -outName hola -outPath .\\PdfVysorWinUI\\Scripts\\ && timeout 25");
        system("cd && timeout 35");
       
        std::filesystem::path path = std::filesystem::current_path();
        Log(path.string());
        /*system("powershell -command Set-ExecutionPolicy AllSigned");
        system("powershell -command Install-Module PSWritePDF -Scope CurrentUser -Force");*/
        //Powershell.Create("get-process").Invoke();
        //system("powershell -command Split-PDF -FilePath C:\\Users\\agomez\\source\\repos\\PdfVysor\\test.pdf -OutputFolder C:\\Users\\agomez\\source\\repos\\PdfVysor\\PdfVysorWinUI\\Scripts\\ && timeout 25");
    }

    void DivideDoc::Log(std::string const& msg) {
        const char* charStr = msg.c_str();
        OutputDebugStringA(charStr);
        OutputDebugStringA("\n");
    }


}



