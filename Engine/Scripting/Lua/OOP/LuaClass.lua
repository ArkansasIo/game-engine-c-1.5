-- Universal Game Engine Lua OOP support.
-- Lua gameplay objects are represented through the engine's script API.

local Class = {}
Class.__index = Class

function Class:new(name, base)
    local cls = setmetatable({ __name = name, __base = base }, self)
    cls.__index = cls
    return cls
end

function Class:instantiate(...)
    local object = setmetatable({}, self)
    if object.initialize then
        object:initialize(...)
    end
    return object
end

function Class:isInstanceOf(target)
    local current = getmetatable(self)
    while current do
        if current == target then return true end
        current = current.__base
    end
    return false
end

return Class
