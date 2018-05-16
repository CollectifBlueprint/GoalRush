;Set team2 meterial
;Transform white pixels to alpha

(define (script-material-post-process inputFilename outputFileName)
   (
      let* (
         (image (car (gimp-file-load RUN-NONINTERACTIVE inputFilename inputFilename)))
         (drawable (car (gimp-image-get-active-layer image)))
		 (img-width (car (gimp-image-width image)))
		 (img-height (car (gimp-image-height image)))
      )
	  
	  (gimp-image-select-rectangle image CHANNEL-OP-ADD (/ img-width 2) 0 (/ img-width 2) img-height)
	  (plug-in-colors-channel-mixer RUN-NONINTERACTIVE image drawable FALSE 1 0 0 0 0 1 0 1 0)
	  
	  (gimp-selection-all image)
      (plug-in-colortoalpha RUN-NONINTERACTIVE image drawable "#FFFFFF")
	  (gimp-context-set-background "#000000")
	  	  
	  (file-png-save2 RUN-NONINTERACTIVE image drawable outputFileName outputFileName 0 9 1 0 0 1 1 1 0)
   )
)