//
// MainPage.xaml.h
// Declaración de la clase MainPage.
//

#pragma once

#include "MainPage.g.h"

namespace PdfVysor
{
	/// <summary>
	/// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

   private:
      Windows::Data::Pdf::PdfDocument^ document;
      int actualPage;

      void Update();
      void ShowPage();

      void OpenFile(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
      void FirstPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
      void PreviousPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
      void NextPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
      void LastPage(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
   };
}
