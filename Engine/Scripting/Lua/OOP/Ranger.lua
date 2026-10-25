local GameObject = require("Engine.Scripting.Lua.OOP.GameObject")

local Ranger = setmetatable({}, { __index = GameObject })
Ranger.__index = Ranger

function Ranger:new(id, name)
    local object = setmetatable({}, self)
    object:initialize(id)
    object.name = name or "Ranger"
    object.bowSkill = 0
    return object
end

function Ranger:attack(target)
    return {
        attacker = self.id,
        target = target,
        skill = self.bowSkill
    }
end

return Ranger
