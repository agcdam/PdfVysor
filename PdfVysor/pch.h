#pragma once
#include <windows.h>
#include <unknwn.h>
#include <restrictederrorinfo.h>
#include <hstring.h>
#include <regex>
#include "winrt/Windows.Foundation.h"
#include "winrt/Windows.Foundation.Collections.h"
#include "winrt/Windows.ApplicationModel.Activation.h"
#include "winrt/Windows.Data.Pdf.h"
#include "winrt/Windows.Storage.h"
#include "winrt/Windows.Storage.Pickers.h"
#include "winrt/Windows.Storage.Streams.h"
#include "winrt/Windows.System.h"
#include "winrt/Windows.UI.Core.h"
#include "winrt/Windows.UI.Xaml.h"
#include "winrt/Windows.UI.Xaml.Automation.Peers.h"
#include "winrt/Windows.UI.Xaml.Controls.h"
#include "winrt/Windows.UI.Xaml.Controls.Primitives.h"
#include "winrt/Windows.UI.Xaml.Documents.h"
#include "winrt/Windows.UI.Xaml.Interop.h"
#include "winrt/Windows.UI.Xaml.Markup.h"
#include "winrt/Windows.UI.Xaml.Media.h"
#include "winrt/Windows.UI.Xaml.Media.Imaging.h"
#include "winrt/Windows.UI.Xaml.Navigation.h"

namespace winrt {
	using namespace winrt::Windows::Data::Pdf;
	using namespace winrt::Windows::Foundation;
	using namespace winrt::Windows::Storage;
	using namespace winrt::Windows::Storage::Streams;
	using namespace winrt::Windows::Storage::Pickers;
	using namespace winrt::Windows::UI::Core;
	using namespace winrt::Windows::UI::Xaml;
	using namespace winrt::Windows::UI::Xaml::Media::Imaging;
	using namespace winrt::Windows::UI::Xaml::Input;
	using namespace winrt::Windows::UI::Xaml::Controls;
}
