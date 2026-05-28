entry   = $8000
resb    = $fffc

src_lo  = $FB
src_hi  = $FC
dst_lo  = $FD
dst_hi  = $FE
cpy_sz  = $FF

f_num = $0200
s_num = $0210

memcpy:
        cpy #$00            

.copy_bytes:
        lda (src_lo),Y      
        sta (dst_lo),Y      
        
        dec cpy_sz
        beq .done
        
        iny                 
        bne .copy_bytes     
.done:
        rts
       
       
; MAIN 
       
	.org entry
	
	jsr memcpy
	stp	

; INIT

	.org resb
	.word entry
	.word $0000
	
	.org f_num
    .byte $bd,$51,$7c,$26

    .org s_num
    .byte $4e,$5c,$f7,$13
    
    .org src_lo
    .word $0200
    
    .org dst_lo
    .word $0220
    
    .org cpy_sz
    .byte $04