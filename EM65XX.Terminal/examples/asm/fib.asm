entry   = $8000
resb    = $fffc

fib_n   = $F0

cpy_sz  = $FA
src_lo  = $FB
src_hi  = $FC
dst_lo  = $FD
dst_hi  = $FE



a = $0000
b = $0010
t = $0020
       
MEMCPY: .macro src, dst, sz
    lda #<\src
    sta src_lo
    lda #>\src
    sta src_hi

    lda #<\dst
    sta dst_lo
    lda #>\dst
    sta dst_hi

    lda \sz
    sta cpy_sz
    jsr memcpy
.endmacro      
 
 
; MAIN 
       
	.org entry
main:			
	lda #$04
    sta cpy_sz

    jsr add4

	MEMCPY b,a,#$04
	MEMCPY t,b,#$04

    dec fib_n
    bne main

    stp
	
; MEMCPY	

memcpy:
	ldy #$00               

.copy_bytes:
    lda (src_lo),Y      
    sta (dst_lo),Y      
    
    dec cpy_sz
    beq .done
    
    iny                 
    bne .copy_bytes     
.done:
    rts

; ADD4

add4:
    ldy #$00
    ldx #$04
    clc

.loop:
    lda a,y
    adc b,y
    sta t,y

    iny
    dex       
    bne .loop 

    rts
    
; INIT

	.org resb
	.word entry
	.word $0000
	
	.org a
    .byte $00,$00,$00,$00

    .org b
    .byte $01,$00,$00,$00
    
    .org t
    .byte $00,$00,$00,$00
    
    .org fib_n
    .byte $2B
    
    .org src_lo
    .word $0000
    
    .org dst_lo
    .word $0000
    
    .org cpy_sz
    .byte $04