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
            var response;
            if (method === "GET") {
                console.log(`${method} ${body} ${headers}`)
                response = await fetch(`http://${window.location.hostname}:8080${url}`, { method, headers })
            }
            else {
                response = await fetch(`http://${window.location.hostname}:8080${url}`, { method, body, headers })
            }            
            if (response.status === 401) {
                localStorage.clear()
                throw new Error("Необходимо авторизоваться!")
            }
            const data = await response.json()
            console.log(data)

            if (!response.ok) throw new Error(data.message)

            setLoading(false)

            return data
        }
        catch (e) {
            setError(e.message)
            setLoading(false)
        }
    }
    
    return [ http, loading, errorState ]
}