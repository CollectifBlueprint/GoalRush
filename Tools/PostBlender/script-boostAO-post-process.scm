;Enhance the AO shadows

(define (script-boostAO-post-process inputFilename outputFileName)
   (
      let* (
         (image (car (gimp-file-load RUN-NONINTERACTIVE inputFilename inputFilename)))
		 (drawable (car (gimp-image-get-active-layer image)))
		 (layer2 (car (gimp-layer-copy drawable FALSE)))
		)
      
	  (gimp-levels drawable 0 169 255 1.80 0 250)
	  (gimp-image-insert-layer image layer2 0 0)
	  (gimp-layer-set-mode layer2 BURN-MODE)
	  
	  (file-png-save2 RUN-NONINTERACTIVE image drawable outputFileName outputFileName 0 9 1 0 0 1 1 1 0)
   )
)