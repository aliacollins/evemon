// EVEMon.WinHook.cpp : Defines the entry point for the DLL application.

#include "stdafx.h"
#include "EVEMon.WinHook.h"
#include <tchar.h>

HANDLE hSem;

void CreateSem();
void DestroySem();
void WindowBeingCreated();

#pragma unmanaged
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		hSem = NULL;
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		DestroySem();
		break;
	}
    return TRUE;
}

void CreateSem()
{
	if (hSem != NULL)
		return;
	hSem = CreateSemaphore(NULL, 0, 1, TEXT("EVEMon-WindowShift-WindowCreation"));
}

void DestroySem()
{
	if (hSem == NULL)
		return;
	CloseHandle(hSem);
	hSem = NULL;
}

void WindowBeingCreated(HWND wnd)
{
	CreateSem();
	if (hSem != NULL)
		ReleaseSemaphore(hSem, 1, NULL);
}

EVEMONWINHOOK_API LRESULT CALLBACK CbtHookProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if ( nCode < 0 )
	{
		return CallNextHookEx(0, nCode, wParam, lParam);
	}
	switch (nCode)
	{
	case HCBT_ACTIVATE:
		{
			LPCBTACTIVATESTRUCT d = (LPCBTACTIVATESTRUCT)lParam;
			WindowBeingCreated(d->hWndActive);
		}
		break;
	case HCBT_CREATEWND:
	case HCBT_MOVESIZE:
	case HCBT_SETFOCUS:
	case HCBT_CLICKSKIPPED:
	case HCBT_DESTROYWND:
	case HCBT_KEYSKIPPED:
	case HCBT_MINMAX:
	case HCBT_QS:
	case HCBT_SYSCOMMAND:
	default:
		break;
	}

	return CallNextHookEx(0, nCode, wParam, lParam);
}