// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the EVEMONWINHOOK_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// EVEMONWINHOOK_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef EVEMONWINHOOK_EXPORTS
#define EVEMONWINHOOK_API __declspec(dllexport)
#else
#define EVEMONWINHOOK_API __declspec(dllimport)
#endif

EVEMONWINHOOK_API LRESULT CALLBACK CbtHookProc(int nCode, WPARAM wParam, LPARAM lParam);

#pragma comment( lib, "user32" )
