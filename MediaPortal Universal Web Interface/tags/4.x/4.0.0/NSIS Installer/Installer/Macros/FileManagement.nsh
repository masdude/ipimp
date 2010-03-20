;---------------
; Setup FileCopy
;---------------
!define FileCopy "!insertmacro FileCopy"
!macro FileCopy FilePath TargetDir
  CreateDirectory "${TargetDir}"
  CopyFiles "${FilePath}" "${TargetDir}"
!macroend

;---------------
; BackupFile
;---------------
!macro BackupFile FILE_DIR FILE
 IfFileExists "$TEMP\*.*" +2
  CreateDirectory "$TEMP"
 IfFileExists "${FILE_DIR}\${FILE}" 0 +2
  Rename "${FILE_DIR}\${FILE}" "$TEMP\${FILE}"
!macroend

;---------------
; RestoreFile
;---------------
!macro RestoreFile RESTORE_TO FILE
 IfFileExists "$TEMP\${FILE}" 0 +2
  Rename "$TEMP\${FILE}" "${RESTORE_TO}\${FILE}"
!macroend