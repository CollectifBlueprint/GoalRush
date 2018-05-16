Center = {
	Name = "Center",
	Components = {
		{
			_class = "SpriteComponent",			
			Parameters = {		
				Sprite = {
					Texture = "@Arenas/JungleArena2/centerPiece.png",
				},
				RenderLayer = "ArenaOverlay3",
			}
		},
		{
			_class = "RigidBodyComponent",			
			Parameters = {			
				Collision = "@Arenas/JungleArena2/centerCollision.png",
			}
		},
	}
}