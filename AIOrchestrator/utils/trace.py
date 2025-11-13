import traceback
from typing import List, Dict, Any

def extract_trace(exc: BaseException, limit: int = 100) -> List[Dict[str, Any]]:
    tb = traceback.TracebackException.from_exception(exc)
    frames = tb.stack[-limit:]
    result = []

    for f in frames:
        # Framework‑internal satırları gizle
        if "site-packages" in f.filename or "Python312" in f.filename:
            continue
        result.append({
            "file": f.filename,
            "line": f.lineno,
            "func": f.name,
            "code": f.line,
        })
        
    result.append({
        "exception": type(exc).__name__,
        "error_msg": str(exc),
    })
    return result
