#pragma once
#include <functional>
#include <vector>

#define HOOK_DECL(name) \
Hook* name ## HooksHead; \
Hook* name ## Hooks; 

#define HOOK_IMPL(nameCapitalized, name) \
inline void add ## nameCapitalized ## Hook(std::function<void()> hook) { \
	if (name ## HooksHead == NULL) { \
		name ## HooksHead = new Hook; \
		name ## Hooks = name ## HooksHead; \
		name ## Hooks->hook = hook; \
	} else { \
		name ## Hooks->next = new Hook; \
		name ## Hooks = name ## Hooks->next; \
		name ## Hooks->hook = hook; \
	} \
} \
inline void run ## nameCapitalized ## Hooks() { \
	Hook* itr = name ## HooksHead; \
	while (itr != NULL) { \
		itr->hook(); \
		itr = itr->next; \
	} \
} 

struct Hook
{
	std::function<void()> hook;
	Hook* next = NULL;
};

class MBXHooks
{
	HOOK_DECL(clientProcess);
	HOOK_DECL(render);
	HOOK_DECL(gameEnd);
	HOOK_DECL(gameStart);
	HOOK_DECL(resolutionChange);

public:
	HOOK_IMPL(ClientProcess, clientProcess);
	HOOK_IMPL(Render, render);
	HOOK_IMPL(GameEnd, gameEnd);
	HOOK_IMPL(GameStart, gameStart);
	HOOK_IMPL(ResolutionChange, resolutionChange);
};

extern MBXHooks gHooks;

#define HOOK_REGISTRATION_IMPL(capitalizedName) \
class capitalizedName ## HookRegistration { \
public: \
	capitalizedName ## HookRegistration(std::function<void()> hook) { \
		gHooks.add ## capitalizedName ## Hook(hook); } };  

HOOK_REGISTRATION_IMPL(ClientProcess);
HOOK_REGISTRATION_IMPL(Render);
HOOK_REGISTRATION_IMPL(GameStart);
HOOK_REGISTRATION_IMPL(GameEnd);
HOOK_REGISTRATION_IMPL(ResolutionChange);

#define MBX_ON_CLIENT_PROCESS(name) \
static void name ## _hook(); \
static ClientProcessHookRegistration name ## _hook_reg(name ## _hook); \
void name ## _hook()

#define MBX_ON_RENDER(name) \
static void name ## _hook(); \
static RenderHookRegistration name ## _hook_reg(name ## _hook); \
void name ## _hook()

#define MBX_ON_GAME_EXIT(name) \
static void name ## _hook(); \
static GameEndHookRegistration name ## _hook_reg(name ## _hook); \
void name ## _hook()

#define MBX_ON_GAME_START(name) \
static void name ## _hook(); \
static GameStartHookRegistration name ## _hook_reg(name ## _hook); \
void name ## _hook()

#define MBX_ON_RESOLUTION_CHANGE(name) \
static void name ## _hook(); \
static ResolutionChangeHookRegistration name ## _hook_reg(name ## _hook); \
void name ## _hook()
