ENTRY = $8000
RESB  = $fffc

a = $00
b = $10
r = $20
    
    .org ENTRY

start:    
    jsr add4
    stp
    
; === ADD4 ===
   
add4:
    ldy #$00
    ldx #$04
    clc

.loop:
    lda a,y
    adc b,y
    sta r,y

    iny
    dex       
    bne .loop 

    rts

; === INIT ===

    .org a
    .byte $87, $D6, $12, $00 ; 1234567

    .org b
    .byte $07, $00, $00, $00
    
    .org r
    .byte $00
    
    .org RESB
    .word start
    .word $0000