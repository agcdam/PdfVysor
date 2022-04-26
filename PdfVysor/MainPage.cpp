#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"


namespace winrt::PdfVysor::implementation
{
	MainPage::MainPage() {
		InitializeComponent();
		
		/*filePathVisor().Text(L"");*/
	}

	void winrt::PdfVysor::implementation::MainPage::nav_Loaded(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e)
	{
		nav().SelectedItem(nav().MenuItems().GetAt(0));
		ContentFram().Navigate(xaml_typename<PdfVysor::Visor>(), *this);
	}


	void winrt::PdfVysor::implementation::MainPage::nav_ItemInvoked(winrt::Windows::UI::Xaml::Controls::NavigationView const& sender, winrt::Windows::UI::Xaml::Controls::NavigationViewItemInvokedEventArgs const& args)
	{

	}


	void winrt::PdfVysor::implementation::MainPage::nav_SelectionChanged(winrt::Windows::UI::Xaml::Controls::NavigationView const& sender, winrt::Windows::UI::Xaml::Controls::NavigationViewSelectionChangedEventArgs const& args)
	{

	}

}









