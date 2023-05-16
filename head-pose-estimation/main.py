"""Demo code showing how to estimate human head pose.

There are three major steps:
1. Detect human face in the video frame.
2. Run facial landmark detection on the face image.
3. Estimate the pose by solving a PnP problem.

To find more details, please refer to:
https://github.com/yinguobing/head-pose-estimation
"""
from argparse import ArgumentParser, ArgumentDefaultsHelpFormatter
import csv
import time

import cv2
import numpy as np

from mark_detector import MarkDetector
from pose_estimator import PoseEstimator

# print(__doc__)
# print("OpenCV version: {}".format(cv2.__version__))

# Parse arguments from user input.
parser = ArgumentParser(usage=__doc__, formatter_class=ArgumentDefaultsHelpFormatter)
parser.add_argument("-v", "--video", type=str, default=None, help="Video file to be processed.")
parser.add_argument("-c", "--cam", type=int, default=None, help="The webcam index.")

parser.add_argument("-i", "--interval", type=float, default=0.2, help="Sampling interval for logging")
parser.add_argument("-u", "--userkey", type=str, default="", help="4 digit user-key to match survery entries")

parser.add_argument("--nosave", action="store_true", help="To disable saving to csv file")
# parser.add_argument("--nopreview", action="store_true", help="To disable the preview window")
args = parser.parse_args()

if __name__ == '__main__':
    # Before estimation started, there are some startup works to do.

    # 1. Setup the video source from webcam or video file.
    video_src = args.cam if args.cam is not None else args.video
    if video_src is None:
        print("Video source not assigned, default webcam will be used.")
        video_src = 0

    cap = cv2.VideoCapture(video_src)

    # Get the frame size. This will be used by the pose estimator.
    width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
    height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
    # 2. Introduce a pose estimator to solve pose.
    pose_estimator = PoseEstimator(img_size=(height, width))
    # 3. Introduce a mark detector to detect landmarks.
    mark_detector = MarkDetector()
    # 4. Measure the performance with a tick meter.
    tm = cv2.TickMeter()

    user_key = args.userkey
    while len(user_key) != 4:
        user_key = input("\nBitte 4 stelligen User-Key eintippen und mit Enter bestätigen:\n")

    estimations = []
    last_save_timestamp = time.time()

    # Now, let the frames flow.
    print("[Started tracking head]")
    while True:

        # Read a frame.
        frame_got, frame = cap.read()
        if frame_got is False:
            break

        # If the frame comes from webcam, flip it so it looks like a mirror.
        if video_src == 0:
            frame = cv2.flip(frame, 2)

        # Step 1: Get a face from current frame.
        facebox = mark_detector.extract_cnn_facebox(frame)

        pose = [[[""]*3]*2]
        # Any face found?
        if facebox is not None:

            # Step 2: Detect landmarks. Crop and feed the face area into the
            # mark detector.
            x1, y1, x2, y2 = facebox
            face_img = frame[y1: y2, x1: x2]

            # Run the detection.
            tm.start()
            marks = mark_detector.detect_marks(face_img)
            tm.stop()

            # Convert the locations from local face area to the global image.
            marks *= (x2 - x1)
            marks[:, 0] += x1
            marks[:, 1] += y1

            # Try pose estimation with 68 points.
            pose = pose_estimator.solve_pose_by_68_points(marks)
            # print(pose)
            # All done. The best way to show the result would be drawing the
            # pose on the frame in realtime.

            # Do you want to see the pose annotation?
            pose_estimator.draw_annotation_box(
                frame, pose[0], pose[1], color=(0, 255, 0))

            # Do you want to see the head axes?
            pose_estimator.draw_axes(frame, pose[0], pose[1])

            axisText = f"x:{pose[0][0]} | y:{pose[0][1]} | z:{pose[0][2]}"
            cv2.putText(frame, axisText, (10, 10), cv2.FONT_HERSHEY_DUPLEX, 0.5, (0, 0, 0), 1)

            # Do you want to see the marks?
            # mark_detector.draw_marks(frame, marks, color=(0, 255, 0))

            # Do you want to see the facebox?
            # mark_detector.draw_box(frame, [facebox])

        now_timestamp = time.time()
        if (now_timestamp - last_save_timestamp) > args.interval:
            # print(now_timestamp - last_save_timestamp)
            estimations.append([now_timestamp, user_key,
                                pose[0][0][0], pose[0][1][0], pose[0][2][0], 
                                pose[1][0][0], pose[1][1][0], pose[1][2][0] ])
            last_save_timestamp = now_timestamp

        # Show preview.
        cv2.imshow("Preview", frame)
        if cv2.waitKey(1) == 27:
            break

    print("[Stopped tracking head]")
    print(f"Average FPS: {tm.getFPS():.2f}")

    if not args.nosave:
        header = ["Timestamp", "Userkey", "rotX", "rotY", "rotZ", "posX", "posY", "posZ"]
        if len(estimations) <= 0:
            exit("No poses logged! Won't create file")
        if len(header) != len(estimations[0]):
            exit("Mismatch in header and rows of CSV file")
            
        with open('pose.csv', 'w', newline='') as f:
            writer = csv.writer(f, delimiter=',', quotechar='"',)
            writer.writerow(header)
            writer.writerows(estimations)

        print("Saved session for", user_key)
