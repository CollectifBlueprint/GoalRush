;Make the arena thumbnail

(define (script-preview-post-process inputFilename outputFileName)
   (
      let* (
         (image (car (gimp-file-load RUN-NONINTERACTIVE inputFilename inputFilename)))
		 (drawable (car (gimp-image-get-active-layer image)))
		)
      
	  (gimp-image-scale image 320 180)
	  (file-png-save2 RUN-NONINTERACTIVE image drawable outputFileName outputFileName 0 9 1 0 0 1 1 1 0) 
   )
)