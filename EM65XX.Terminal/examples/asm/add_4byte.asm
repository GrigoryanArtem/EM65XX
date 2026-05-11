ENTRY = $8000
RESB  = $fffc

F_NUM = $00
S_NUM = $04
R_NUM = $10
    
    .org ENTRY

start:
    clc
    ldx #$00
loop:
    jsr add
    inx
    cpx #$04
    beq exit
    jmp loop

add:
    lda F_NUM,x
    adc S_NUM,x

    sta R_NUM,x
    rts

exit:
    stp

    .org RESB
    .word start
    .word $0000

    .org F_NUM
    .byte $bd,$51,$7c,$26

    .org S_NUM
    .byte $4e,$5c,$f7,$13