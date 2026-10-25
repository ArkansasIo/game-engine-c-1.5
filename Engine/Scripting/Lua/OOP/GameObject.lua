local Class = require("Engine.Scripting.Lua.OOP.LuaClass")

local GameObject = Class:new("GameObject")

function GameObject:initialize(id)
    self.id = id
    self.components = {}
end

function GameObject:addComponent(name, component)
    self.components[name] = component
end

function GameObject:getComponent(name)
    return self.components[name]
end

return GameObject
