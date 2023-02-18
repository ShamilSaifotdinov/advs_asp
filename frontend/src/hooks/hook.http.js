import { useState } from 'react'

export default function useQuery() {
    const [ loading, setLoading ] = useState(false)
    const [ errorState, setError ] = useState(null)

    const http = async (url, method="GET", body={}, headers={}) => {
        if (errorState) {
            setError(null)
        }
        setLoading(true)

        try {
          const response = await fetch(`http://${window.location.hostname}:8080${url}`, { method, body, headers })
          if (!response.ok) {
            throw new Error ("Ошибка")
          }

          const data = response.json()

          setLoading(false)

          return data
        }
        catch (e) {          
          setError(e)
          setLoading(false)
        }
    }
    
    return [ http, loading, errorState ]
}