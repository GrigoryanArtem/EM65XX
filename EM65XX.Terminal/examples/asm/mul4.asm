ENTRY = $8000
RESB  = $fffc

a = $00
b = $10
r = $20
counter = $F0
    
    .org ENTRY

start:    
    jsr mul4
    stp
    
; === MUL4 ===

mul4:
	ldy #$00
    ldx #$04
    
    lda #$00
    sta r+0
    sta r+1
    sta r+2
    sta r+3
    
	clc

	lda #32
    sta counter

.loop		
	lda a+0
    and #$01
    
    beq .shift
    
    jsr add4
	
.shift
	lsr a+3
    ror a+2
    ror a+1
    ror a+0
    
    asl b+0
    rol b+1
    rol b+2
    rol b+3

    dec counter
    bne .loop

    rts
    
    
; === ADD4 ===
   
add4:
    ldy #$00
    ldx #$04
    clc

.loop:
    lda r,y
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
    .byte $00, $00, $00, $00
    
    .org counter
    .byte $20
    
    .org RESB
    .word start
    .word $0000