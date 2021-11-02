
from locust import HttpUser, between, task
import json


class TestExamplesApi(HttpUser):
    wait_time = between(1, 2)

    @task
    def test_with_ring_buffer(self):
        self.client.get(url="with/ting-tuffer", verify=False)

    @task
    def test_without_ring_buffer(self):
        self.client.get(url="without/ting-tuffer", verify=False)
