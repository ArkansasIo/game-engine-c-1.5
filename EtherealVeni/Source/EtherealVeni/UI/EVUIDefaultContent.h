#pragma once

class UEVUISubsystem;

/**
 * Registers the game's complete interface through the public UEVUISubsystem API:
 * micro menu, Game Menu, Options tree + settings, key bindings, Help & Support panel and the
 * right-click dropdowns (player, target, minimap, chat, system). Treat it as the reference for
 * adding your own menus.
 */
namespace EVUIDefaultContent
{
	void Register(UEVUISubsystem& UI);
}
