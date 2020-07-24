for _,force in pairs(game.forces) do
	if force.technologies["laser-turrets"] then force.technologies["laser-turrets"].enabled = true end
end