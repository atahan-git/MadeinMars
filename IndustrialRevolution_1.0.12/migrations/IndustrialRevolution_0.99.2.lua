-- repair changes to quickbar unlocks
for _,force in pairs(game.forces) do
	if force.technologies["deadlock-bronze-construction"].researched or force.technologies["personal-roboport-equipment"].researched then
		for _,player in pairs(force.players) do
			for _,shortcut in pairs(game.shortcut_prototypes) do
				if shortcut.technology_to_unlock == "personal-roboport-equipment" then player.set_shortcut_available(shortcut.name,true) end
			end
		end
	end
end