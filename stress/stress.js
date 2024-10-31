
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        { duration: '1m', target: 300 },      
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(95)<2000'], // 95% of requests should be below 200ms
    }
};

export default function () {
    const response = http.get('http://127.0.0.1:58499/api/Produto');
    
    check(response, {
        'is status 200': (x) => x.status === 200
    });
    
    sleep(1);
}