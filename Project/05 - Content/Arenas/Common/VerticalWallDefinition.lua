Wall = {
	Name = "Wall",
	Components = {
		{
			_class = "SpriteComponent",	
			Parameters = {		
				Sprite = { Texture = "@Arenas/Common/VerticalWall.png" },
				RenderLayer = "ArenaOverlay1",
			}
		},		
		{
			_class = "RigidBodyComponent",	
			Parameters = {			
				Collision = "@Arenas/Common/VerticalWallCollision.png",
			}
		}
	}
}
