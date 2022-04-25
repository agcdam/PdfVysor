#include "pch.h"
#include "Visor.xaml.h"
#if __has_include("Visor.g.cpp")
#include "Visor.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::PdfVysorWinUI::implementation
{
    Visor::Visor()
    {
        InitializeComponent();
        this->NavigationCacheMode(NavigationCacheMode::Enabled);
        
        Log("Pagina cargada------");
        
        
        //PageControllersVisibility(Visibility::Collapsed);
    }

    //----------------------------------------------------------------------------------------------------------
    //Open file controls
    //----------------------------------------------------------------------------------------------------------

    fire_and_forget Visor::OpenFileButton_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        OpenFile().IsEnabled(false);

        HWND hwnd = GetActiveWindow();

        FileOpenPicker picker;
        auto initializeWithWindow{ picker.as<::IInitializeWithWindow>()};
        initializeWithWindow->Initialize(hwnd);

        picker.FileTypeFilter().Append(L".pdf");
        StorageFile file = co_await picker.PickSingleFileAsync();

        PdfDocument pdf = nullptr;
        if (file != nullptr)
        {
            
            try
            {
                pdf = co_await PdfDocument::LoadFromFileAsync(file);
                Log("Pdf renderizado-------------------");
            }
            catch (winrt::hresult_error const& ex)
            {
                switch (ex.to_abi())
                {
                case __HRESULT_FROM_WIN32(ERROR_WRONG_PASSWORD):
                    //error contraseña, informar de que esta protegido
                    break;

                case E_FAIL:
                    //error documento es incorrecto
                    break;

                default:
                    // error I/O reportado como excepcion
                    break;
                }
            }

        }

        if (pdf != nullptr)
        {
            NewPdfLoaded(pdf);
        }
        //establecer en visible si el archivo ha sido abierto correctamente 
        //o si ha dado error pero ya habia un archivo
        if (m_pdfDocument != nullptr)
        {
            //asignar a m_pdfDocument el nuevo documento
            PageControllersVisibility(Visibility::Visible); 
            TotalPages().Content(winrt::box_value(winrt::to_hstring(m_pdfDocument.PageCount())));
            ActualPage().Content(winrt::box_value(winrt::to_hstring(m_actualPage + 1)));
            
            CheckPageControls();
            //renderizar la pagina
            //mostrar la pagina
        }
        
        OpenFile().IsEnabled(true);
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Set the enviroment to receive new document
    */
    void Visor::NewPdfLoaded(PdfDocument const& newDoc)
    {
        m_pdfDocument = nullptr;
        m_pdfDocument = newDoc;
        m_actualPage = 0;
        m_zoomScroller = 100;
        m_pagesCache.clear();
        
    }

    //----------------------------------------------------------------------------------------------------------
    //Page navigation controls
    //----------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------
    void Visor::FirstPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        //establecer la pagina actual en 0
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Navigates to the previous page
    */
    void Visor::PreviousPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        //restar 1 a la pagina actual si es posible
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Navigates to the next page
    */
    void Visor::NextPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        //sumar 1 a la pagina actual si es posible
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Navigates to the last page
    */
    void Visor::LastPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        //navegar a la ultima pagina
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Search the page written
    */
    void Visor::SearchPage_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        winrt::hstring string = InputPageBox().Text();
        //FlyoutPage().Hide();
        if (IsNumber(string) && IsInRange(string))
        {
            CheckPageControls();
            ActualPage().Content(winrt::box_value(winrt::to_hstring(m_actualPage + 1)));
            FlyoutPage().Hide();

        }
        else
        {
            InputPageBox().Text(winrt::to_hstring(m_actualPage + 1));
        }

    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Check if the page given is a number
    */
    bool Visor::IsNumber(winrt::hstring const& str)
    {
        std::string string = winrt::to_string(str);
        std::regex regex("^[0-9]*$");
        if (std::regex_match(string, regex) && !string.empty())
        {
            return true;
        }
        return false;

    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Check if the page given is in of pages range
    */
    bool Visor::IsInRange(winrt::hstring const& str)
    {
        int newPage = std::stoi(winrt::to_string(str)); //get new page in index 1
        if (newPage > 0 && newPage <= m_pdfDocument.PageCount())
        {
            m_actualPage = (newPage - 1); //set actual page in index 0
            return true;
        }
        return false;
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Check if the page controls need to be blocked or not
    */
    void Visor::CheckPageControls()
    {
        if (m_actualPage == 0) //actual page is the first
        {
            PreviousPage().IsEnabled(false);
            FirstPage().IsEnabled(false);
            NextPage().IsEnabled(true);
            LastPage().IsEnabled(true);
        }
        else if (m_actualPage == (m_pdfDocument.PageCount() - 1)) //actual page is the last
        {
            PreviousPage().IsEnabled(true);
            FirstPage().IsEnabled(true);
            NextPage().IsEnabled(false);
            LastPage().IsEnabled(false);
        }
        else //actual page is in range without be the first or the last
        {
            PreviousPage().IsEnabled(true);
            FirstPage().IsEnabled(true);
            NextPage().IsEnabled(true);
            LastPage().IsEnabled(true);
        }
    }


    //----------------------------------------------------------------------------------------------------------
    //Zoom controls
    //----------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------
    /*
        Decrease zoom level 10% every time button is clicked
    */
    void Visor::ZoomOut_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        if (m_zoomScroller > (kminZoom + 10))
        {
            m_zoomScroller -= 10;
        }
        else if (m_zoomScroller > kminZoom)
        {
            m_zoomScroller -= m_zoomScroller - kminZoom;
        }
        ZoomValue().Content(winrt::box_value(winrt::to_hstring(m_zoomScroller) + L" %"));
        //establecer el zoom en el scrollviewer
        
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Set zoom level based on a slider when zoom indicator is clicked
    */
    void Visor::Slider_ValueChanged(winrt::IInspectable const& sender, winrt::Primitives::RangeBaseValueChangedEventArgs const& e)
    {
        m_zoomScroller = (int) e.NewValue();
        ZoomValue().Content(winrt::box_value(winrt::to_hstring(m_zoomScroller) + L" %"));
        //establecer el zoom en el scrollviewer
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Increase zoom level 10% every time button is clicked
    */
    void Visor::ZoomIn_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        if (m_zoomScroller < (kmaxZoom -10))
        {
            m_zoomScroller += 10;
        }
        else if (m_zoomScroller < kmaxZoom)
        {
            m_zoomScroller += kmaxZoom - m_zoomScroller;
        }
        ZoomValue().Content(winrt::box_value(winrt::to_hstring(m_zoomScroller) + L" %"));
        //establecer el zoom en el scrollviewer
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Sets zoom level to 100%
    */
    void Visor::ResetZoom_Click(winrt::IInspectable const& sender, winrt::RoutedEventArgs const& e)
    {
        m_zoomScroller = 100;
        ZoomValue().Content(winrt::box_value(winrt::to_hstring(m_zoomScroller) + L" %"));
        //establecer el zoom en el scrollviewer
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Set actual zoom value to the slider selector
    */
    void Visor::FlyoutZoom_Opening(winrt::IInspectable const& sender, winrt::IInspectable const& e)
    {
        ZoomSelector().Value(m_zoomScroller);
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Update content every time scroller page change
    */
    void Visor::ScrollerPage_ViewChanging(winrt::IInspectable const& sender, winrt::ScrollViewerViewChangingEventArgs const& e)
    {
        Log("scroller changing");
        m_zoomScroller = (int) (ScrollerPage().ZoomFactor() * 100);
        ZoomValue().Content(winrt::box_value(winrt::to_hstring(m_zoomScroller) + L" %"));
    }

    //----------------------------------------------------------------------------------------------------------
    //Utility
    //----------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------
    /*
        Show logs while debug
    */
    void Visor::Log(std::string const& msg)
    {
        const char* charStr = msg.c_str();
        OutputDebugStringA(charStr);
        OutputDebugStringA("\n");
    }

    //----------------------------------------------------------------------------------------------------------
    /*
        Change visibility of the page controllers like navigation or zoom
    */
    void Visor::PageControllersVisibility(winrt::Visibility const& visibility)
    {
        //establecer la visibilidad de los controles de pagina
        FirstPage().Visibility(visibility);
        PreviousPage().Visibility(visibility);
        ActualPage().Visibility(visibility);
        TotalPages().Visibility(visibility);
        NextPage().Visibility(visibility);
        LastPage().Visibility(visibility);
        ZoomOut().Visibility(visibility);
        ZoomValue().Visibility(visibility);
        ZoomIn().Visibility(visibility);
        ResetZoom().Visibility(visibility);
        Separator1().Visibility(visibility);
        Separator2().Visibility(visibility);
    }

    

    
    
}